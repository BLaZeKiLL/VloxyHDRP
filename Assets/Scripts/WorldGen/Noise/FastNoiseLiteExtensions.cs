namespace CodeBlaze.Vloxy.Game {
    public static class FastNoiseLiteExtensions {

        public static FastNoiseLite FromProfile(FastNoiseLiteProfile profile) {
            var fnl = new FastNoiseLite();

            fnl.SetSeed(profile.Seed);
            fnl.SetNoiseType(profile.NoiseType);
            fnl.SetFrequency(profile.Frequency);
            fnl.SetFractalType(profile.FractalType);
            fnl.SetFractalOctaves(profile.FractalOctaves);
            fnl.SetFractalLacunarity(profile.FractalLacunarity);
            fnl.SetFractalGain(profile.FractalGain);  

            return fnl;
        }

    }
}