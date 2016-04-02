angular
  .module('starter')
  .controller('CBase', function ($scope)
  {
    $scope.items = [];

    $scope.CarregaMais = function() {
      //Carrega 20 itens
      for (var i = 0; i < 20; i++) {
        $scope.items.push('Item - ' + ($scope.items.length + 1));
      }
      $scope.$broadcast('scroll.infiniteScrollComplete');
    };

  });
