angular.module('starter')
.service('DB', function ($cordovaSQLite) {

  this.insere = function (dado) {
    var query = "INSERT INTO teste (Nome, Idade) VALUES (?,?)";
    $cordovaSQLite.execute(db, query, [dado.nome, dado.idade]).then(function(res) {
        console.log("Dado inserido");
    }, function (err) {
        console.error(err);
    });
  }

  this.listar = function (funcSuccess, funcError) {
    var query = "SELECT * FROM teste";
    $cordovaSQLite.execute(db, query, []).then(funcSuccess, funcError);
  };

  this.limparTabela = function () {
    var query = "Delete from teste";
    $cordovaSQLite.execute(db, query, []).then(function(res) {
        console.log("Tabela Limpa");
    }, function (err) {
        console.error(err);
    });
  }

});
