(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('ViewPerson', Person);

  Person.$inject = ['person', 'activities', '$stateParams', '$state'];

  /* @ngInject */
  function Person(person, activities, $stateParams, $state) {
    var vm = this;

    activate();

    function activate() {
      vm.person = person;
      vm.activities = activities;
    }
  }
})();
