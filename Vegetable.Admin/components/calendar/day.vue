<template>
    <v-col>
        <v-col>
            <v-card>
                <v-container container--fluid grid-list-md >
                    <v-row style="width: 110px;">
                        <v-col d-flex>
                            <v-card height="3em" ml-4 flat>
                                <v-col display-2>{{daydate.format("D")}}</v-col>
                            </v-card>
                        </v-col>
                        <v-col d-flex>
                            <v-row wrap>
                                <v-col xs1 sm2 md1>
                                    <v-card height="2em" flat>
                                        <v-col subheading>{{daydate | weekDayName()}}</v-col>
                                    </v-card>
                                </v-col>
                            </v-row>
                        </v-col>
                    </v-row>
                </v-container>
            </v-card>
        </v-col>
        <v-col v-for="(timeOffset, index) in DayOffsets" :key='index'>
            <v-card :class="{'timecell' :timeOffset.IsShow}" height="1.5em" ml-4>
            </v-card>
        </v-col>
    </v-col>
</template>

<script>

export default {
  props: {
    daydate: {}
  },

  computed: {
    DayOffsets () {
      var now = new Date()
      var hoursPeriods = ['00', '30']
      var times = []
      for (var i = 0; i < 24; i++) {
        for (var j = 0; j < 2; j++) {
          var time = i + ':' + hoursPeriods[j]
          var isShow = j !== 1
          now.setHours(i)
          now.setMinutes(hoursPeriods[j])
          times.push(
            {
              Time: time, IsShow: isShow, DisplayTime: now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
            })
        }
      }
      return times
    }
  },

  methods: {
  },

  filters: {
    weekDayName (day) {
      return day.localeData().weekdays()[day.day()]
    }
  }
}
</script>

<style>  

.timecell{
  border-bottom: 0.5px solid;
  border-radius:1px; 
}
</style>