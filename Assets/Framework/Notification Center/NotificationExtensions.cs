using UnityEngine;
using System;
using System.Collections;
using Handler = System.Action<System.Object, System.Object>;

public static class NotificationExtensions
{
	public static void PostNotification (this object obj, string notificationName)
	{
		NotificationCenter.instance.PostNotification(notificationName, obj);
	}
	
	public static void PostNotification (this object obj, string notificationName, object e)
	{
		NotificationCenter.instance.PostNotification(notificationName, obj, e);
	}

    public static void AddObserver(this object obj, string notificationName, Handler handler)
	{
		NotificationCenter.instance.AddObserver(handler, notificationName);
	}

    public static void AddObserver(this object obj, string notificationName, Handler handler, object sender)
	{
		NotificationCenter.instance.AddObserver(handler, notificationName, sender);
	}

    public static void RemoveObserver(this object obj, string notificationName, Handler handler)
	{
		NotificationCenter.instance.RemoveObserver(handler, notificationName);
	}

    public static void RemoveObserver(this object obj, string notificationName, Handler handler, System.Object sender)
	{
		NotificationCenter.instance.RemoveObserver(handler, notificationName, sender);
	}
}