<template>
    <v-row>
        <v-dialog v-model="dialog" max-width="300px">
            <template v-slot:activator="{ on }">
                <v-avatar v-on="on" :color="color"> </v-avatar>
            </template>
            <v-card>
                <v-card-title>Select Color</v-card-title>
                <v-divider></v-divider>
                <v-card-text>
                    <v-container>
                        <v-row>
                            <v-col cols="12" md="3" v-for="item in colors" :key="item">
                                <v-avatar :color="item" v-on:click.native="selectColor(item)"></v-avatar>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-row>
</template>

<script>

 var colorList = [
            "red",
            "pink",
            "purple",
            "deep-purple",
            "indigo",
            "blue",
            "light-blue",
            "cyan",
            "teal",
            "green",
            "light-green",
            "lime",
            "yellow",
            "amber",
            "orange",
            "deep-orange",
            "blue-grey",
            "grey"
        ];

export default {
      data(context) {
        return {
            colors: colorList,
            dialog: false,
            color: this.currentColor
        }
    },
    methods: {
         selectColor(color) {
             this.$emit('colorSelected', color)
             this.color = color;
             this.dialog = false;
         }
    },
    props: ['currentColor']
}
</script>