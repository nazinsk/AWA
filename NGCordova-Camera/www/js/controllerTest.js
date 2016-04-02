angular.module('starter')
.controller("controllerTest", function ($scope, $cordovaCamera) {

  $scope.tirarFoto = function () {
      var options = {
        quality: 50,
        destinationType: Camera.DestinationType.DATA_URL,
        sourceType: Camera.PictureSourceType.CAMERA,
        encodingType: Camera.EncodingType.JPEG,
        targetWidth: 100,
        targetHeight: 100,
        saveToPhotoAlbum: false,
  	    correctOrientation:true
      };

      $cordovaCamera.getPicture(options).then(
        function(imageData) {
          $scope.imagem = "data:image/jpeg;base64," + imageData;
        },
        function(err) {
          // error
        });
  };

});
