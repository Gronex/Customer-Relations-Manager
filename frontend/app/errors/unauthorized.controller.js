(function(){
  'use strict';

  angular
    .module("CRM")
    .controller("Unauthorized", Controller);

  Controller.$inject = [];
  function Controller(){
    var vm = this;

    activate();

    function activate(){
      vm.error = "Unauthorized";
      vm.message = "You do not have the rights to do that.";
    }
  }
})();
