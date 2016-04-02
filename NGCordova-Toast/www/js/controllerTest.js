angular.module('starter')
.controller("controllerTest", function ($scope, $cordovaToast) {
  $scope.mostrarToast = function () {
    $cordovaToast.showLongBottom('Mostrando mensagem');
  };
});
