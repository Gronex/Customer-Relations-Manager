(function() {
  'use strict';

  angular
      .module('CRM')
      .controller('People', People);

  People.$inject = ['$state', 'people'];

  /* @ngInject */
  function People($state, people) {
    var vm = this;
    vm.query = {};
    vm.itemCount = 0;
    vm.headers = [
      {
        label: "First name",
        selector: "firstName",
        type: "link",
        link: "People.view"
      },
      {
        label: "Last name",
        selector: "lastName",
        type: "link",
        link: "People.view"
      },
      {
        label: "Email",
        selector: "email",
        type: "email"
      },
      {
        label: "Phone number",
        selector: "phoneNumber"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "People.edit"
      }
    ];

    vm.getPeople = getPeople;

    activate();

    function activate() {
      getPeople();
      setupData(people);
    }

    function getPeople(sortParam) {
      _.merge(vm.query, {orderBy: sortParam});
      $state.go("People.list", vm.query);
    }

    function setupData(data) {
      vm.itemCount = data.itemCount;
      vm.people = data.data;
      _.merge(vm.query, {
        pageSize: data.pageSize,
        page: data.pageNumber,
        orderBy: data.query.orderBy
      });
    }
  }
})();
