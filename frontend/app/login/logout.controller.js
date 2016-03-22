(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Logout', Logout);

  Logout.$inject = ['authorization', '$state'];
  function Logout(authorization, $state){
    authorization
      .logout();
    $state.go("Home");
  }
})();
