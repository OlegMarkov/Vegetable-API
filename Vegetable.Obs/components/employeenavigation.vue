<template>
  <li v-on:click.stop.prevent class="uk-text-center uk-text-truncate">
    <a class="obs-tab-link" v-on:click="resetEmployee" v-if="$store.state.selectedEmployee !=  null">{{title}}</a>
    <a href="#" v-else>{{ title }}</a>

  </li>
</template>

<script>
  export default {

    props: ['index'],

    computed: {
      title() {
        return this.$store.state.selectedEmployee == null ? this.$i18n.t('obs.navigation_employee') : this.$store.state.selectedEmployee.firstName;
      }
    },

    methods: {
      resetEmployee: function () {
        this.$store.commit('changeEmployee', null);
        this.$store.commit('changeDate', null);
        this.$store.commit('changeTime', null);
        this.$store.commit('changeClient', null);
        UIkit.tab('#steps').show(this.index);
      }
    }
  }
</script>
