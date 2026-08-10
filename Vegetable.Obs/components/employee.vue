<style>

div.obs-employee:hover{
  cursor: pointer;
}

</style>

<template>
  <li>
    <div class="">
      <h3 id="obs-employee-title" class="uk-text-center">{{ $t('obs.employee_title') }}</h3>
      <div class="uk-child-width-1-3@m uk-grid-small uk-grid-match uk-flex-center" uk-grid>
        <div v-for="employee in $store.state.owner.employees" :key="employee.id">
          <div class="uk-card uk-card-default uk-card-hover uk-card-body obs-employee obs-card" v-on:click="employeeSelected(employee)">
            <div class="uk-grid-small uk-flex-middle" uk-grid>
              <div class="uk-width-auto">
                <img class="uk-border-circle" width="40" height="40" src="~/assets/images/employee.jpg">
              </div>
              <div class="uk-width-expand">
                <h4 class="uk-margin-remove-bottom">{{employee.firstName}} {{employee.lastName}}</h4>
              </div>
            </div>
            <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit.</p>
          </div>
        </div>
      </div>
    </div>
  </li>
</template>


<script>

  export default {

    props: ['index'],

    methods: {
      employeeSelected: function (employee) {
        this.scrollClass = '';
        this.$store.commit('changeEmployee', employee);
        UIkit.tab('#steps').show(this.index + 1);
      }
    }

  }

</script>
