angular.module('instawa')
.factory("USUARIOS", function($http, API_URL) {

  return {
    _login: _login,
    _salva: _salva
  };

  function _login(email, senha) {
    return $http.post(API_URL + 'usuarios/login/?email=' + email + '&senha='+senha);
  }

  function _salva(usuarioDados) {
    return $http.post(API_URL + 'usuarios/', usuarioDados);
  }

})
