(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('ViewPerson', Person);

  Person.$inject = ['person', 'activities'];

  /* @ngInject */
  function Person(person, activities) {
    var vm = this;

    activate();

    function activate() {
      vm.person = person;
      vm.activities = activities;
    }
  }
})();
