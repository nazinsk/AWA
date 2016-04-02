angular.module('starter')
.controller("controllerTest", function ($scope, $cordovaToast, $cordovaFile) {
  $scope.gravar = function () {
    $cordovaFile.writeFile(cordova.file.dataDirectory, "DATA.txt", "Data: " + new Date(), true)
          .then(function (success) {
            // success
            $cordovaToast.showLongBottom('Gravado com sucesso');
          }, function (error) {
            // error
            $cordovaToast.showLongBottom('Erro ao gravar');
          });
  };
  $scope.ler = function () {
    $cordovaFile.readAsText(cordova.file.dataDirectory, "DATA.txt")
          .then(function (success) {
            // success
            $cordovaToast.showLongBottom('Lido com sucesso');
            $scope.conteudoArquivo = success;
          }, function (error) {
            $cordovaToast.showLongBottom('Erro ao ler');
            // error
          });

  };
});
