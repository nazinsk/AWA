angular.module('instawa')
.controller('LoginCtrl', function ($scope, $rootScope, $state, $cordovaToast, USUARIOS, SEGURANCA, $ionicLoading){

  $scope.dadosLogin = {};

  $scope.login = function() {

    if(!$scope.dadosLogin.email){
      $cordovaToast.showLongBottom('Informe seu e-mail');
      return;
    }
    else if(!$scope.dadosLogin.senha){
      $cordovaToast.showLongBottom('Informe sua senha');
      return;
    }

    $ionicLoading.show();
    USUARIOS._login($scope.dadosLogin.email, $scope.dadosLogin.senha)
      .then(
        function(res) {
          SEGURANCA._gravaLogin(res.data);
          $state.go('principal');
          $ionicLoading.hide();
          $rootScope.$emit('login');
        },
        function(res) {
          console.log(res);
          $cordovaToast.showLongBottom('Usuário ou senha incorretos, tente novamente.');
          $ionicLoading.hide();
        }
    );
  }
});
