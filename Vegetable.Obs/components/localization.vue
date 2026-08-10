<style>
  .obs-languages {
    position: absolute;
    top: 10px;
    left: 5%;
  }

  .obs-languages .uk-dropdown {
    min-width: 0%;
  }

</style>
<template>

  <div class="obs-languages">
    <button class="uk-button uk-button-default" type="button">{{ $store.state.locale }}</button>
    <div uk-dropdown="mode: click; pos: bottom-justify">
      <ul class="uk-nav uk-dropdown-nav">
        <li v-bind:class="{ 'uk-active': locale == $store.state.locale }"  v-for="locale in $store.state.locales" :key="locale">
          <a href="#" v-on:click="localeSelected(locale)">{{ locale }}</a>
        </li>
      </ul>
    </div>
  </div>

</template>
<script>

export default {

    methods: {
      localeSelected: function (locale) {
        this.$store.commit('changeLocale', locale);
      }
    }

  }

</script>
