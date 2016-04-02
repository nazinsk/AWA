angular.module('starter')
.controller('cep', CEP);

function CEP($http, $scope) {

  $scope.cep = '';

  $scope.buscaCep = function () {

    if($scope.cep){

      //http://api.postmon.com.br/v1/cep/{CEP}
      $http.get('http://api.postmon.com.br/v1/cep/' + $scope.cep)
      .then(
        function (dados) {
          //Sucesso
          console.info('SUCESSO');
          console.log(dados);

          $scope.texto = JSON.stringify(dados.data);

        },
        function (data) {
          //Erro
          console.info('ERRO');
          console.log(data);
        }
      );
    }


  }



}
