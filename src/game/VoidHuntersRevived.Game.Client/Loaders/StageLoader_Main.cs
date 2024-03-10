namespace VoidHuntersRevived.Game.Client.Loaders
{
    /*
    [AutoLoad]
    internal sealed class StageLoader_Main : IStageLoader
    {
        public BlockList StageBlockList { get; } = BlockList.CreateWhitelist(Stages.Main);

        public void Load(Stage main)
        {
            main.StyleSheet.Set<Unit>(Property.Width, Selector.Create<Stage>(), 1f)
                .Set(Property.Padding, Selector.Create<Label>("test"), new Padding(15, 15, 15, 15))
                .Configure(Selector.Create<Element>(), manager =>
                {
                    manager.Set(Property.Padding, new Padding(5, 5, 5, 5))
                        .Set(Property.Alignment, Alignment.TopCenter);
                })
                .Configure(Selector.Create<Label>(), manager =>
                {
                    manager.Set(Property.Inline, true)
                        .Set(Property.Font, Resources.Fonts.Default)
                        .Set(Property.Color, Color.Red)
                        .Set(Property.Color, ElementState.Hovered, Color.Green)
                        .Set(Property.BackgroundColor, ElementState.Hovered, Color.LightBlue);
                });
        }
    }
    */
}
