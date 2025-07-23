 var MyPlugin = {
     IsMobile: function()
     {
         return UnityLoader.SystemInfo.mobile;
     },
	 
	 Redirect: function(url)
	 {
		url = Pointer_stringify(url);
		window.open(url);
	 },
 };
 
 mergeInto(LibraryManager.library, MyPlugin);