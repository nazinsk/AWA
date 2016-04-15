angular.module('instawa')
.factory("POSTS", function($http, API_URL) {
  return {
    _postar: _postar,
    _listarFeed: _listarFeed,
    _listarUsuario: _listarUsuario,
    _excluir: _excluir
  };

  function _listarFeed(pagina) {
    return $http.get(API_URL + 'posts?pagina=' + pagina);
  }

  function _listarUsuario(usuarioID) {
    return $http.get(API_URL + 'posts/usuario/' + usuarioID);
  }

  function _postar(postDados) {
    return $http.post(API_URL + 'posts', postDados);
  }

  function _excluir(postID) {
    return $http.delete(API_URL + 'posts/' + postID);
  }
})
