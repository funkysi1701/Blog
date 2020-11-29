var JsFunctions = window.JsFunctions || {};

JsFunctions = {
    setDocumentTitle: function (title) {
        document.title = title;
        var meta = document.createElement('meta');
        meta.name = "twitter:card";
        meta.content = "summary";
        document.getElementsByTagName('head')[0].appendChild(meta);
        var meta = document.createElement('meta');
        meta.name = "twitter:title";
        meta.content = title.trim();
        document.getElementsByTagName('head')[0].appendChild(meta);
        var meta = document.createElement('meta');
        meta.name = "twitter:description";
        meta.content = title.trim();
        document.getElementsByTagName('head')[0].appendChild(meta);
        var meta = document.createElement('meta');
        meta.name = "twitter:url";
        meta.content = window.location.href;
        document.getElementsByTagName('head')[0].appendChild(meta);
        var meta = document.createElement('meta');
        meta.name = "twitter:site";
        meta.content = "@funkysi1701";
        document.getElementsByTagName('head')[0].appendChild(meta);
        var meta = document.createElement('meta');
        meta.name = "twitter:image";
        meta.content = "https://www.funkysi1701.com/wp-content/uploads/2014/09/1922276.jpg";
        document.getElementsByTagName('head')[0].appendChild(meta);
    }
};