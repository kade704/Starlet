public static class StarterSaveFile
{
    public static string StarterSaveFileData = @"
{
    ""gameState"": {
        ""station"": ""Procyon"",
        ""coin"": 100,
        ""position"": [0, 0]
    },
    ""inventory"": [
        {
            ""name"": ""HullA1"",
            ""amount"": 5
        },
        {
            ""name"": ""Engine1"",
            ""amount"": 2
        },
        {
            ""name"": ""Thruster1"",
            ""amount"": 4
        },
        {
            ""name"": ""Gun1"",
            ""amount"": 2
        },
        {
            ""name"": ""Miner1"",
            ""amount"": 2
        },
        {
            ""name"": ""Storage1"",
            ""amount"": 2
        }
    ],
    ""hangarManager"": {
        ""currentHangarIndex"": 0,
        ""hangarsData"": [
            {
                ""name"": ""초보자 함선"",
                ""spacecraftData"": [
                    {
                        ""name"": ""Base"",
                        ""parent"": -1,
                        ""connector"": -1,
                        ""properties"": []
                    },
                    {
                        ""name"": ""HullA1"",
                        ""parent"": 0,
                        ""connector"": 0,
                        ""properties"": []
                    },
                    {
                        ""name"": ""Storage1"",
                        ""parent"": 0,
                        ""connector"": 1,
                        ""properties"": []
                    },
                    {
                        ""name"": ""Engine1"",
                        ""parent"": 0,
                        ""connector"": 2,
                        ""properties"": [
                            {
                                ""id"": ""SignalProperty:입력신호"",
                                ""value"": ""C""
                            }
                        ]
                    },
                    {
                        ""name"": ""HullA1"",
                        ""parent"": 0,
                        ""connector"": 3,
                        ""properties"": []
                    },
                    {
                        ""name"": ""Thruster1"",
                        ""parent"": 1,
                        ""connector"": 0,
                        ""properties"": [
                            {
                                ""id"": ""SignalProperty:입력신호"",
                                ""value"": ""A""
                            }
                        ]
                    },
                    {
                        ""name"": ""Gun1"",
                        ""parent"": 2,
                        ""connector"": 0,
                        ""properties"": [
                            {
                                ""id"": ""SignalProperty:입력신호"",
                                ""value"": ""D""
                            }
                        ]
                    },
                    {
                        ""name"": ""Thruster1"",
                        ""parent"": 4,
                        ""connector"": 2,
                        ""properties"": [
                            {
                                ""id"": ""SignalProperty:입력신호"",
                                ""value"": ""B""
                            }
                        ]
                    }
                ],
                ""controllersData"": [
                    {
                        ""name"": ""PushButton"",
                        ""x"": 707.5145,
                        ""y"": -271.5607,
                        ""properties"": [
                            {
                                ""id"": ""SignalProperty:출력신호"",
                                ""value"": ""D""
                            }
                        ]
                    },
                    {
                        ""name"": ""VerticalSlider"",
                        ""x"": -801.1561,
                        ""y"": -248.670532,
                        ""properties"": [
                            {
                                ""id"": ""SignalProperty:출력신호"",
                                ""value"": ""C""
                            }
                        ]
                    },
                    {
                        ""name"": ""HorizontalJoystick"",
                        ""x"": -503.583832,
                        ""y"": -261.1561,
                        ""properties"": [
                            {
                                ""id"": ""SignalProperty:왼쪽출력신호"",
                                ""value"": ""A""
                            },
                            {
                                ""id"": ""SignalProperty:오른쪽출력신호"",
                                ""value"": ""B""
                            }
                        ]
                    }
                ]
            }
        ]
    }
}";
}