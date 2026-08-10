<template>
  <li v-on:click.stop.prevent class="uk-text-center">
    <a class="obs-tab-link" v-on:click="resetAddress" v-if="$store.state.selectedAddress !=  null">{{title}}</a>
    <a href="#" v-else>{{ title }}</a>
    </li>
</template>


<script>

export default{

  props: ['index'],

  computed: {
    title(){
      return this.$store.state.selectedAddress ==  null ? this.$i18n.t('obs.navigation_location') : this.$store.state.selectedAddress.state;
    }
  }
}

</script>
