angular
  .module('starter')
  .controller('CBase', function ($scope)
  {
    $scope.onTouch = function() {
      console.log('TOUCH');
    }

    $scope.onTap = function() {
      console.log('TAP');
    }

    $scope.onClick = function() {
      console.log('CLICK');
    }

    $scope.onSwipeLeft = function() {
      console.log('TO LEFT');
    }

    $scope.onSwipeRight = function() {
      console.log('TO RIGHT');
    }


  });
