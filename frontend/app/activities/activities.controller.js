(function(){
  'use strict';

  angular
    .module('CRM')
    .controller('Activities', Activities);

  Activities.$inject = ['dataservice'];
  function Activities(dataservice){
    var vm = this;
    vm.pagination = {
      page: 1,
      pageSize: 10
    };
    vm.itemCount = 0;
    vm.headers = [
      {
        label: "Name",
        selector: "name"
      },
      {
        label: "Company",
        selector: "companyName"
      },
      {
        label: "Owner",
        selector: "primaryResponsibleName"
      },
      {
        label: "Contact",
        selector: "primaryContactName"
      },
      {
        label: "Due",
        selector: "dueDate",
        format: "date"
      },
      {
        label: "From",
        selector: "dueTimeStart",
        format: "time"
      },
      {
        label: "To",
        selector: "dueTimeEnd",
        format: "time"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "Person"
      }
    ];

    vm.getActivities = getActivities;
    vm.sort = sort;

    activate();

    function activate(){
      getActivities();
    }

    function getActivities(){
      return dataservice.activities
        .get({query: vm.pagination})
        .then(setupData);
    }

    function sort(selector){
      var cpy = angular.copy(vm.pagination);
      cpy.orderBy = selector;
      dataservice.activities
        .get({query: cpy})
        .then(setupData);
    }

    function setupData(data){
      vm.itemCount = data.itemCount;
      vm.activities = _.map(data.data, function(a){
        a.dueDate = moment.utc(a.dueDate);
        a.dueDate.local();
        if(a.dueTimeStart){
          a.dueTimeStart = moment.utc(a.dueTimeStart);
          a.dueTimeStart.local();
        }
        if(a.dueTimeEnd){
          a.dueTimeEnd = moment.utc(a.dueTimeEnd);
          a.dueTimeEnd.local();
        }
        return a;
      });
    }
  }
})();
