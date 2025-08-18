mergeInto(LibraryManager.library, {

    GetTokenFromLocalStorage: function () {
        try {
            var token = window.localStorage.getItem('authToken');
            if (token) {
                var bufferSize = lengthBytesUTF8(token) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(token, buffer, bufferSize);
                return buffer;
            }
        } catch (e) {
            console.error('LocalStorage 토큰 조회 오류:', e);
        }
        return null;
    },

    SetTokenToLocalStorage: function (token) {
        try {
            var tokenStr = UTF8ToString(token);
            window.localStorage.setItem('authToken', tokenStr);
            console.log('LocalStorage에 토큰 저장 완료');
        } catch (e) {
            console.error('LocalStorage 토큰 저장 오류:', e);
        }
    },

    RemoveTokenFromLocalStorage: function () {
        try {
            window.localStorage.removeItem('authToken');
            console.log('LocalStorage에서 토큰 삭제 완료');
        } catch (e) {
            console.error('LocalStorage 토큰 삭제 오류:', e);
        }
    }

});