using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;



public class ConfigConverter
{
    


#region Config Packing

    [MenuItem("Assets/打包配置文件")]
    static void packConfig()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selection != null && selection.Length > 0)
        {
            foreach (var obj in selection)
            {
               PackFile(obj);
            }     
        }
    }


    
    [MenuItem("Assets/打包配置文件", true)]
    static bool ValidatepackConfig()
    {
        foreach (UnityEngine.Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            
            if (!IsValidatePath(path))
                return false;
        }
        return true;
    }


    //是否有效的配置文件路径
    public static bool IsValidatePath(string path)
    {
        string extension = Path.GetExtension(path);
        string dir = Path.GetDirectoryName(path);

        if (!extension.Equals(".txt") || !dir.Equals(PathDefine.DIR_CONFIG))
            return false;

        return true;
    }

    public static void PackFile(UnityEngine.Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        PackFile(path);
    }

    public static void PackFile(string filePath)
    {
        string content = File.ReadAllText(filePath);

        string className = Path.GetFileNameWithoutExtension(filePath);

        string cs = Tool.makeCsString(className, content);
        if (!Directory.Exists(PathDefine.DIR_CONFIG))
            Directory.CreateDirectory(PathDefine.DIR_CONFIG);
        string outputFileName = PathDefine.DIR_BEAN_SCRIPT_OUTPUT + "/" + className + "Bean.cs";
        File.WriteAllText(outputFileName, cs, Encoding.UTF8);
        Debug.Log("complete:" + outputFileName);

    }


#endregion


    class Tool
    {
        public static string makeCsString(string tableName, string content)
        {

            string[] lines = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 3)
            {
                System.Console.WriteLine(tableName + " is not enough 3 lines ");
                return null;
            }

            string fieldNames = lines[0].Trim(new char[] { ' ', '\r', '\t', '"' });       //第一行字段名称集合
            string commentNames = lines[1].Trim(new char[] { ' ', '\r', '\t', '"' });     //第二行字段名称集合
            string typeNames = lines[2].Trim(new char[] { ' ', '\r', '\t', '"' });        //第三行数据类型集合

            if (string.IsNullOrEmpty(fieldNames) || string.IsNullOrEmpty(commentNames) || string.IsNullOrEmpty(typeNames))
            {
                Debug.LogError(tableName + " fieldNames commentNames typeNames is Empty！");
                return null;
            }

            string[] fieldlist = fieldNames.Split(new char[] { '\t' });
            string[] commentlist = commentNames.Split(new char[] { '\t' });
            string[] typelist = typeNames.Split(new char[] { '\t' });
            Bean.DataType[] dataTypeList = Array.ConvertAll<string, Bean.DataType>(typelist, (d) =>
            {
                return Bean.Convertor.Instance.Str2Type(d);
            });

            if (dataTypeList == null)
            {
                Debug.LogError(tableName + " fieldNames commentNames typeNames length is not same！");
                return null;
            }

            if (fieldlist.Length != commentlist.Length || fieldlist.Length != typelist.Length)
            {
                Debug.LogError(tableName + " fieldNames commentNames typeNames length is not same！" + fieldlist.Length + " " + commentlist.Length + " " + typelist.Length);
                return null;
            }

            /***********生成过程***********/

            tableName = tableName + "Bean";

            string mu = makeUsing(dataTypeList);
            string gs = makeFields(dataTypeList, fieldlist, commentlist);
            string ms = makeParse(tableName, dataTypeList, fieldlist, commentlist);
            StringBuilder builder = new StringBuilder();
            builder.Append(mu);
            builder.Append("public class ").Append(tableName).Append(":Bean").Append(Constants.NEW_LINE);
            builder.Append("{").Append(Constants.NEW_LINE);
            builder.Append(gs).Append(Constants.NEW_LINE);
            builder.Append(ms).Append(Constants.NEW_LINE);
            builder.Append("}");

            return builder.ToString();
        }

        public static string makeUsing(Bean.DataType[] types)
        {
            StringBuilder builder = new StringBuilder();   
            builder.Append("using System;").Append(Constants.NEW_LINE);
            builder.Append("using System.Collections;").Append(Constants.NEW_LINE);
            builder.Append("using UnityEngine;").Append(Constants.NEW_LINE);
            builder.Append("using System.Collections.Generic;").Append(Constants.NEW_LINE);

            return builder.ToString();
        }

        public static string makeFields(Bean.DataType[] types, string[] fields, string[] comments)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < types.Length; i++)
            {
                string type = Bean.Convertor.Instance.Type2Str(types[i]);
                string field = fields[i];
                string commment = comments[i];

                builder.Append(Constants.TAB_1).Append("public ").Append(type).Append(Constants.SPACE).Append(field).Append(Constants.SPACE);
                builder.Append(Constants.TAB_1).Append("{ ").Append("get; private set;").Append(" }");
                builder.Append(Constants.TAB_1).Append("/*").Append(commment).Append(" */").Append(Constants.NEW_LINE);
                builder.Append(Constants.NEW_LINE);
            }

            return builder.ToString();
        }

        public static string makeParse(string className, Bean.DataType[] types, string[] fields, string[] comments)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(Constants.TAB_1);
            builder.Append("public static Dictionary<int, ").Append(className).Append("> beans = new Dictionary<int, ").Append(className).Append(">();").Append(Constants.NEW_LINE);
            builder.Append(Constants.NEW_LINE);


            builder.Append(Constants.TAB_1).Append("public ").Append(className).Append("()").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_1).Append("{").Append(Constants.NEW_LINE);
            builder.Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_1).Append("}").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_1).Append("public ").Append(className).Append(" this[int index]").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_1).Append("{").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_2).Append("get { return beans[index]; }").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_1).Append("}").Append(Constants.NEW_LINE);


            builder.Append(Constants.TAB_1);
            builder.Append("public static void Init()").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_1);
            builder.Append("{").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_2);
            builder.Append("Type tp =  typeof(").Append(className).Append(");").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_2);
            builder.Append("string tableName = tp.ToString().ToLower().Replace(\"bean\", \"\");").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_2);
            builder.Append("List<List<string>> values = TableLists[tableName];").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_2);
            builder.Append("for(int i=0;i<values.Count;i++)").Append("{").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_3);
            builder.Append("List<string> line = values[i];").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_3);
            builder.Append(className).Append(" bean").Append(" = ").Append("new ").Append(className).Append("();").Append(Constants.NEW_LINE);

            for (int j = 0; j < types.Length; j++)
            {
                string type = Bean.Convertor.Instance.Type2Str(types[j]);
                string field = fields[j];
                string commment = comments[j];

                builder.Append(Constants.TAB_3);         
                string lineValue = "line["+j+"]";
                string result = "ParseData<" + type + ">(DataType." + types[j] + ", " + lineValue + ");";
                builder.Append("bean.").Append(field).Append(" = ").Append(result);
                builder.Append(Constants.TAB_1).Append("/*").Append(commment).Append(" */").Append(Constants.NEW_LINE);
            }

            builder.Append(Constants.TAB_3);
            builder.Append("beans[bean.ID] = bean;").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_2);
            builder.Append("}").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_1);
            builder.Append("}").Append(Constants.NEW_LINE);


            builder.Append(Constants.TAB_1).Append("public static ").Append(className).Append(" Get(int index)").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_1).Append("{").Append(Constants.NEW_LINE);

            builder.Append(Constants.TAB_2).Append("if (beans.ContainsKey(index))").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_2).Append("{").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_3).Append(" return beans[index];").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_2).Append("}").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_2).Append("Debug.LogError(\"Table index error:[").Append(className).Append("]:\"+index);").Append(Constants.NEW_LINE);
            builder.Append(Constants.TAB_2).Append("return null;").Append(Constants.NEW_LINE);
            
            builder.Append(Constants.TAB_1).Append("}").Append(Constants.NEW_LINE);


            return builder.ToString();

        }
    }

   
   
    class Constants
    {
        public static string NEW_LINE = "\r\n";

        public static string NEW_LINE_2 = "\r\n\r\n";

        public static string TAB_1 = "  ";
        public static string TAB_2 = "      ";
        public static string TAB_3 = "          ";
        public static string TAB_4 = "              ";
        public static string TAB_5 = "                  ";
        public static string TAB_6 = "                      ";

        public static string SPACE = " ";

        public static string EMPTY = "";
    }
}
