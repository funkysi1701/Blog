var JsFunctions = window.JsFunctions || {};

JsFunctions = {
    setDocumentTitle: function (title) {
        document.title = title;
        var meta = document.createElement('meta');
        meta.name = "twitter:title";
        meta.content = title.trim();
        document.getElementsByTagName('head')[0].appendChild(meta);
        var meta = document.createElement('meta');
        meta.name = "twitter:url";
        meta.content = window.location.href;
        document.getElementsByTagName('head')[0].appendChild(meta);
    }
};