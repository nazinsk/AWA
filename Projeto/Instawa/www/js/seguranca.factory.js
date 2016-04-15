angular.module('instawa')
.factory("SEGURANCA", function($rootScope) {

  return {
    _gravaLogin: _gravaLogin,
    _efetuarLogoff: _efetuarLogoff,
    _estaLogado: _estaLogado,
    _dadosUsuarioLogado: _dadosUsuarioLogado
  };

  function _estaLogado() {
    leDados();
    if($rootScope.usuarioLogadoDados){
      return true
    }
    else {
      return true
    }
  }

  function _dadosUsuarioLogado() {
    leDados();
    return $rootScope.usuarioLogadoDados;
  }

  function _gravaLogin(usuarioDados) {
    $rootScope.usuarioLogadoDados = usuarioDados;
    salvaDados($rootScope.usuarioLogadoDados);
  }

  function _efetuarLogoff() {
    $rootScope.usuarioLogadoDados = undefined;
    salvaDados(undefined);
  }


  function salvaDados(dados) {
    if(dados)
      localStorage.setItem("dadosLogin", JSON.stringify(dados));
    else
      localStorage.removeItem("dadosLogin");
  }

  function leDados() {
    if(!$rootScope.usuarioLogadoDados){
      $rootScope.usuarioLogadoDados = JSON.parse(localStorage.getItem('dadosLogin'));
      if($rootScope.usuarioLogadoDados){
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return true;
    }
  }

});
