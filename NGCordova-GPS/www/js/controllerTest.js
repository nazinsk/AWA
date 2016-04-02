angular.module('starter')
.controller("controllerTest", function ($scope, $cordovaGeolocation) {

  $scope.getLocalizacao = function () {
    var posOptions = {timeout: 10000, enableHighAccuracy: false};
    
    $cordovaGeolocation
      .getCurrentPosition(posOptions)
      .then(function (position) {
        $scope.localizacao = position.coords.latitude + ' - ' + position.coords.longitude;
      }, function(err) {
        // error
      });
  };

});
