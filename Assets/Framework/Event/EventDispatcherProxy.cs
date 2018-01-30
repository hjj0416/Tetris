using System;

//namespace TheNextMoba.Module.Arena
//{
	public interface EventDispatcherProxy
	{
		EventDispatcher Dispatcher { get;}
	}
	public static class EventDispatcherProxyExtension
	{
		public static void SendEvent(this EventDispatcherProxy proxy, short type)
		{
			proxy.Dispatcher.SendEvent (type);
		}

		public static void SendEvent<T>(this EventDispatcherProxy proxy, short type, T msg)
		{
			proxy.Dispatcher.SendEvent<T> (type, msg);
		}

		public static void AddEventHandler(this EventDispatcherProxy proxy, short type, EventHandler handler)
		{
            proxy.Dispatcher.AddEventHandler(type, handler);
		}

        public static void AddEventHandler<T>(this EventDispatcherProxy proxy, short type, EventHandler<T> handler)
		{
			proxy.Dispatcher.AddEventHandler<T> (type, handler);
		}

        public static void RemoveEventHandler(this EventDispatcherProxy proxy, short type, EventHandler handler)
		{
			proxy.Dispatcher.RemoveEventHandler(type, handler);
		}

        public static void RemoveEventHandler<T>(this EventDispatcherProxy proxy, short type, EventHandler<T> handler)
		{
			proxy.Dispatcher.RemoveEventHandler<T> (type, handler);
		}

        public static void AddOneShotEventHandler(this EventDispatcherProxy proxy, short type, EventHandler handler)
        {
            proxy.Dispatcher.AddOneShotEventHandler(type, handler);
        }

        public static void RemoveOneShotEventHandler(this EventDispatcherProxy proxy, short type, EventHandler handler)
        {
            proxy.Dispatcher.RemoveOneShotEventHandler(type, handler);
        }

        public static void AddOneShotEventHandler<T>(this EventDispatcherProxy proxy, short type, EventHandler<T> handler)
		{
			proxy.Dispatcher.AddOneShotEventHandler<T> (type, handler);
		}

        public static void RemoveOneShotEventHandler<T>(this EventDispatcherProxy proxy, short type, EventHandler<T> handler)
		{
			proxy.Dispatcher.RemoveOneShotEventHandler<T> (type, handler);
		}
	}
//}

