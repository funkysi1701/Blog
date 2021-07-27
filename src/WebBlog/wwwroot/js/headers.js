var JsFunctions = window.JsFunctions || {};

JsFunctions = {
    setDocumentTitle: function (title) {
        document.title = title;
    },
    offset: function offset() {
        return new Date().getTimezoneOffset() / 60;
    }
};
