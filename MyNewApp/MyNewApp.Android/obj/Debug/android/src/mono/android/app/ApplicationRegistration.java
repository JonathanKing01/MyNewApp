package mono.android.app;

public class ApplicationRegistration {

	public static void registerApplications ()
	{
				// Application and Instrumentation ACWs must be registered first.
		mono.android.Runtime.register ("MyNewApp.Droid.MainApplication, MyNewApp.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", md5ffe7d75bc4792d70fa4d0f47e728bc7e.MainApplication.class, md5ffe7d75bc4792d70fa4d0f47e728bc7e.MainApplication.__md_methods);
		
	}
}
