<template>
  <li v-on:click.stop.prevent class="uk-text-center uk-text-truncate">
    <a class="obs-tab-link" v-on:click="resetDate" v-if="$store.state.selectedDate !=  null">{{title}}</a>
    <a href="#" v-else>{{ title }}</a>
  </li>
</template>

<script>
  export default {

    props: ['index'],

    computed: {
      title() {
        return this.$store.state.selectedDate == null ? this.$i18n.t('obs.navigation_date_time') : this.$store.state.selectedDateTime;
      }
    },
    methods: {
      resetDate: function () {
        this.$store.commit('changeDate', null);
        this.$store.commit('changeTime', null);
        this.$store.commit('changeClient', null);
        UIkit.tab('#steps').show(this.index);
      }
    }
  }
</script>
