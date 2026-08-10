<template>

  <li v-on:click.stop.prevent class="uk-active uk-text-center uk-text-truncate">
    <a class="obs-tab-link" v-on:click="resetService" v-if="$store.state.selectedService !=  null">{{title}}</a>
    <a class="" href="#" v-else>{{ title }}</a>
  </li>
</template>

<script>

  export default {

    props: ['index'],

    computed: {
      title() {
        return this.$store.state.selectedService == null ? this.$i18n.t('obs.navigation_service') : this.$store.state.selectedService.title;
      }
    },

    methods: {
      resetService() {
        this.$store.commit('changeService', null);
        this.$store.commit('changeEmployee', null);
        this.$store.commit('changeDate', null);
        this.$store.commit('changeTime', null);
        this.$store.commit('changeClient', null);
        UIkit.tab('#steps').show(this.index);
      }
    }
  }
</script>
