angular
  .module('starter')
  .controller('Home', function($scope, $ionicModal, $ionicPopup, $ionicActionSheet){

  $ionicModal.fromTemplateUrl('views/modal.html', {
    scope: $scope,
    animation: 'slide-in-up'
  }).then(function(modal) {
    $scope.modal = modal;
  });

  $scope.abreModal = function(){
    $scope.modal.show();
  }

  $scope.fecharModal = function(){
    $scope.modal.hide();
  }

  $scope.abrePopup = function(){
    // An elaborate, custom popup
    $ionicPopup.show({
      template: 'CONTEÚDO',
      title: 'Solictação',
      subTitle: 'Uso pontual',
      scope: $scope,
      buttons: [
        { text: 'Cancelar' },
        { text: 'OK',
          onTap: function(e) {
            console.log('Tapped1!', e);
            return "OK";
          }
        }
      ]
    }).then(function(res) {
      console.log('Tapped!', res);
    });
  }

  $scope.abreActionSheet = function(){

    $ionicActionSheet.show({
      buttons: [
        { text: '<b>Testar</b> isso' },
        { text: 'Apenas ver' }
      ],
      destructiveText: 'NADA',
      titleText: 'O que quer fazer?',
      cancelText: 'ESQUECE',
      cancel: function() {
           console.log('ESQUECE');
         },
     destructiveButtonClicked: function() {
          console.log('NADA');
          return true; // Para fechar
        },
      buttonClicked: function(index) {
        console.log('Selecionado:' + index );
        return true; // Para fechar
      }
    });
  }




});
