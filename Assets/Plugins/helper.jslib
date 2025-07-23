mergeInto(LibraryManager.library, {
    RedirectDomain: function(check_domains_str, redirect_domain) {
        var redirect = true;
        var domains_string = Pointer_stringify(check_domains_str);
        var redirect_domain_string = Pointer_stringify(redirect_domain);
        var check_domains = domains_string.split("|");
        for (var i = 0; i < check_domains.length; i++) {
            var domain = check_domains[i];
            if(document.location.host == domain) 
            {
                redirect = false;
            }
        }
        if(redirect) 
        { 
            document.location = redirect_domain_string; 
            return true;
        }
        return false;
    },
     
    IsMobile: function () {
        return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
    },
     
    InvokeFunction: function(func_name) {
        var func_name_str = Pointer_stringify(func_name);
        var func = window[func_name_str];
        if (func) {
            func();
        }
    },
     
    OpenNewTab : function(url)
    {
        url = Pointer_stringify(url);
        window.open(url,'_blank');
    },
});