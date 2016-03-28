(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('App', App);

  App.$inject = ['authorization', 'navbar'];
  function App(authorization, navbar){
    var vm = this;

    vm.user = undefined;
    vm.logout = logout;

    activate();

    function activate() {
      authorization.subscribe(function(){
        vm.navbar = navbar.generate();
      });
      authorization.login();
    }

    function logout() {
      authorization.logout();
      vm.user = undefined;
    }

  }
})();
