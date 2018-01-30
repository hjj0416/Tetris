using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleObject : MonoBehaviour
{
    public List<GameObject> activate;
	public List<GameObject> deactivate;

	[HideInInspector][SerializeField] GameObject target;
	[HideInInspector][SerializeField] bool inverse = false;

	void Awake ()
	{
		if (target != null)
		{
			if (activate.Count == 0 && deactivate.Count == 0)
			{
				if (inverse) deactivate.Add(target);
				else activate.Add(target);
			}
			else target = null;
		}

#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
        Toggle toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggle);
	    OnToggle(toggle.isOn);
	}

	public void OnToggle (bool flag)
	{
		if (enabled)
		{
			for (int i = 0; i < activate.Count; ++i)
                Set(activate[i], flag);
			for (int i = 0; i < deactivate.Count; ++i)
                Set(deactivate[i], !flag);
		}
	}

	void Set (GameObject go, bool state)
	{
		if (go != null)
		{
			go.SetActive(state);
		}
	}
	
}
