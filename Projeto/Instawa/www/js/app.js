// Ionic Starter App

// angular.module is a global place for creating, registering and retrieving Angular modules
// 'starter' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
angular.module('instawa', ['ionic', 'ngCordova'])

.run(function($ionicPlatform) {
  $ionicPlatform.ready(function() {
    if(window.cordova && window.cordova.plugins.Keyboard) {
      // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
      // for form inputs)
      cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);

      // Don't remove this line unless you know what you are doing. It stops the viewport
      // from snapping when text inputs are focused. Ionic handles this internally for
      // a much nicer keyboard experience.
      cordova.plugins.Keyboard.disableScroll(true);
    }
    if(window.StatusBar) {
      StatusBar.styleDefault();
    }
  });

  var push = new Ionic.Push({
    "debug": true,
    "onNotification": function(notification) {
      var payload = notification.payload;
      console.log(notification, payload);
    },
    "onRegister": function(data) {
      console.log('TOKEN:'+ data.token);
      push.saveToken(data.token);
    },
    "pluginConfig": {
      "ios": {
        "badge": true,
        "sound": true,
        "alert": true
       }
    }
  });

  push.register();
})

//Rotas
.config(function($stateProvider, $urlRouterProvider) {
  'use strict';

  $stateProvider

  .state('login', {
    url: '/login',
    templateUrl: 'views/login.html',
    controller: 'LoginCtrl'
  })
  .state('cadastro', {
    url: '/cadastro',
    templateUrl: 'views/cadastro.html',
    controller: 'CadastroCtrl'
  })

  .state('principal', {
    url: '/principal',
    templateUrl: 'views/principal.html',
    controller: 'PrincipalCtrl'
  })

  $urlRouterProvider.otherwise('/login');

})
.constant('API_URL', 'http://apiprojeto.academiawebapps.com/api/')
;
