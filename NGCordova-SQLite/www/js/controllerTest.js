angular.module('starter')
.controller("controllerTest", function ($scope, DB) {

  $scope.insere = function () {
    DB.insere({nome: 'teste', idade: 23});
  }

  $scope.listar = function () {
    $scope.dados = [];
    DB.listar(function(res) {
        for (i = 0; i < res.rows.length; i++) {
          $scope.dados.push(res.rows.item(i));
        }
    }, function (err) {
        console.error(err);
    });
  };

  $scope.limparTabela = function() {
    DB.limparTabela();
  }
});
