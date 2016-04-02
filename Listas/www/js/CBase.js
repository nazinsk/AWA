angular
  .module('starter')
  .controller('CBase', function ($scope)
  {
    $scope.dadosLista = [];

    $scope.dadosLista = [
      {nome:"Usuário 1", endereco:"Rua teste,123 - SP - BR"},
      {nome:"Usuário 2", endereco:"Rua teste,123 - SP - BR"},
      {nome:"Usuário 3", endereco:"Rua teste,123 - SP - BR"}
    ];

    $scope.CLICK = function(obj) {
      console.log(obj);
    }

  });
