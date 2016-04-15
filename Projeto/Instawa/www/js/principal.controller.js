angular.module('instawa')
.controller('PrincipalCtrl', function ($scope, $rootScope){
  $scope.selecionaTab = function (index) {
    $rootScope.$emit('tab' + index);
  }
});
