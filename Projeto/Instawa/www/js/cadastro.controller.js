angular.module('instawa')
.controller('CadastroCtrl', function ($scope, $rootScope, $state, USUARIOS, SEGURANCA, $cordovaToast, $ionicLoading){

  $scope.dadosUsuario = {
    nome: '',
    email: '',
    senha: '',
  };

  $scope.cadastrar = function() {
    if(!$scope.dadosUsuario.nome){
      $cordovaToast.showLongBottom('Informe seu nome');
      return;
    }
    if(!$scope.dadosUsuario.email){
      $cordovaToast.showLongBottom('Informe seu e-mail');
      return;
    }
    if(!$scope.dadosUsuario.senha){
      $cordovaToast.showLongBottom('Informe sua senha');
      return;
    }

    $ionicLoading.show();

    USUARIOS._salva($scope.dadosUsuario)
      .then(
        function(res) {
          SEGURANCA._gravaLogin(res.data);
          $state.go('principal');
          $ionicLoading.hide();
          $rootScope.$emit('login');
        },
        function(res) {
          $cordovaToast.showLongBottom('Ocorreu um erro, tente novamente.');
          $ionicLoading.hide();
        }
    );
  }
});
