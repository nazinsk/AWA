angular.module('instawa')
.controller('FeedCtrl', function ($scope, POSTS, $ionicPopup, $ionicScrollDelegate){

  $scope.posts = [];
  var pagina = 0;
  $scope.semMaisDados = false;

  $scope.scrollTop = function() {
    $ionicScrollDelegate.scrollTop();
  };

  $scope.mostrarModalUsuario = function(dados) {
    $scope.dadosUsuario = dados;

    $ionicPopup.show({
      templateUrl: 'views/perfilUsuario.html',
      title: 'Que é?',
      scope: $scope,
      buttons: [
        { text: 'OK' }
      ]
    });

  };


  $scope.atualizarDoInicio = function() {
    $scope.posts = [];
    pagina = 0;
    $scope.semMaisDados = false;
    $scope.atualizar();
  };

  $scope.atualizar = function() {

    pagina++;

    POSTS._listarFeed(pagina)
    .then(
      function(res) {
        if(res.data.length > 0){
          angular.forEach(res.data, function(value, key) {
            $scope.posts.push(value);
          });
        }
        else {
          $scope.semMaisDados = true;
        }
        $scope.$broadcast('scroll.infiniteScrollComplete');
        $scope.$broadcast('scroll.refreshComplete');
      },
      function(res) {
        $cordovaToast.showLongBottom('Erro ao buscar as postagens.');
        $scope.$broadcast('scroll.infiniteScrollComplete');
        $scope.$broadcast('scroll.refreshComplete');
      }
    );
  }

});
