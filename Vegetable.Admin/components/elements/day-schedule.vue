<template>
  <v-row>
    <v-col xs12 sm6 md2>
      <v-btn fab small
      :class= "[ schedule.active ? 'theme--dark deep-purple' : 'deep-purple lighten-5']"
      @click="daySelect()">{{ schedule.dayTitle }}</v-btn>
    </v-col>
    <v-col xs12 sm6 md2>
      <time-picker :title="timeTitleFrom" :currentTime.sync="schedule.workTime.from" @update:currentTime="$emit('update:workTime', schedule.workTime)"></time-picker>
    </v-col>
    <v-col xs12 sm6 md2>
      <time-picker :title="timeTitleTo" :currentTime.sync="schedule.workTime.to" @update:currentTime="$emit('update:workTime', schedule.workTime)"></time-picker>
    </v-col>
    <v-spacer></v-spacer>
    <v-col xs12 sm6 md2>
      <time-picker :title="timeTitleFrom" :currentTime.sync="schedule.breakTime.from" @update:currentTime="$emit('update:breakTime', schedule.breakTime)"></time-picker>
    </v-col>
    <v-col xs12 sm6 md2>
      <time-picker :title="timeTitleTo" :currentTime.sync="schedule.breakTime.to" @update:currentTime="$emit('update:breakTime', schedule.breakTime)"></time-picker>
    </v-col>
    <v-spacer></v-spacer>
  </v-row>
</template>

<script>

import TimePicker from '~/components/elements/timepicker.vue'

export default {
  data () {
    return {
      schedule: {
        dayTitle: this.dayTitle,
        active: this.active,
        workTime: {
          from: this.workTime.from,
          to: this.workTime.to
        },
        breakTime: {
          from: this.breakTime.from,
          to: this.breakTime.to
        }
      },

      timeTitleFrom: 'From:',
      timeTitleTo: 'To:'
    }
  },

  components: {
    TimePicker
  },

  props: {
    dayTitle: {},
    active: {},
    workTime: {},
    breakTime: {}
  },

  computed: {

  },

  methods: {
    daySelect () {
      let current = this.schedule.active
      this.schedule.active = !current
      this.$emit('update:active', this.schedule.active)
    }
  }
}
</script>