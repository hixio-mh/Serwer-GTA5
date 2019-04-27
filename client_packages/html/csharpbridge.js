window.onerror = (message, source, lineno, colno, error) => {
    callCSharp("onError", message, source, lineno, colno, error);
};

function callCSharp() {
    if (typeof mp !== "undefined") {
        mp.trigger("CEF->C#", ...arguments);
    }
    else {
        console.log("CEF->C#", ...arguments);
    }
}

function testowa(a, b, c, d) {
    print("asdf a = " + a + " b = " + b);
}

function onCSharp(base64) {
    var serializedString = window.atob(base64);
    var obj = JSON.parse(serializedString);
    var funcName = obj[0];
    if (window[funcName]) {
        window[funcName](obj[1], obj[2], obj[3], obj[4], obj[5], obj[6], obj[7], obj[8], obj[9], obj[10]);
    }
}

function print(str) {
    callCSharp("print", str);
}
function test() {
    callCSharp("test", "bla1", "bla2", "bla3");
}

