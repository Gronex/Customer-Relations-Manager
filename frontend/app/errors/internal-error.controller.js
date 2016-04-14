(function(){
  'use strict';

  angular
    .module("CRM")
    .controller("InternalError", Controller);

  Controller.$inject = [];
  function Controller(){
    var vm = this;

    activate();

    function activate(){
      vm.error = "Internal error";
      vm.message = "Something whent critically wrong. Please contact a system admin and explain what you did when the error happened.";
    }
  }
})();
