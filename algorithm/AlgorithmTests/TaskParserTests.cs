using Algorithm.Algorithm;
using Algorithm.Communication;
using Algorithm.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTests
{
    public class TaskParserTests
    {
        [Fact]
        public void SimpleTest()
        {
            var numberOfFlats = 0;
            var numberOfSharps = 0;
            var meterCount = 3;
            var meterValue = 4;

            var tonation = Tonation.GetTonation(numberOfSharps, numberOfFlats);
            var meter = Meter.GetMeter(meterValue, meterCount);

            var quarterNote = RhytmicValue.QUARTER_NOTE;
            var halfNoteDotted = RhytmicValue.HALF_NOTE_DOTTED;

            Note C_1_S = new("C", 5, Voice.SOPRANO);
            Note E_1_A = new("E", 4, Voice.ALTO);
            Note G_1_T = new("G", 3, Voice.TENORE);
            Note C_1_B = new("C", 3, Voice.BASS);

            Note C_2_S = new("C", 5, Voice.SOPRANO);
            Note F_2_A = new("F", 4, Voice.ALTO);
            Note A_2_T = new("A", 3, Voice.TENORE);
            Note C_2_B = new("C", 3, Voice.BASS);

            Note B_3_S = new("B", 4, Voice.SOPRANO);
            Note G_3_A = new("E", 4, Voice.ALTO);
            Note G_3_T = new("G", 3, Voice.TENORE);
            Note D_3_B = new("C", 3, Voice.BASS);

            Note C_4_S = new("C", 5, Voice.SOPRANO);
            Note E_4_A = new("E", 4, Voice.ALTO);
            Note G_4_T = new("G", 3, Voice.TENORE);
            Note C_4_B = new("C", 3, Voice.BASS);

            Symbol T = new(minor: false, functionSymbol: FunctionSymbol.T);
            Symbol S = new(minor: false, functionSymbol: FunctionSymbol.T);
            Symbol D = new(minor: false, functionSymbol: FunctionSymbol.T);

            Function function1 = new(symbol: T, isMain: true);
            Function function2 = new(symbol: S, isMain: true);
            Function function3 = new(symbol: D, isMain: true);
            Function function4 = new(symbol: T, isMain: true);

            UserStack stack1 = new(function1, tonation, 0);
            UserStack stack2 = new(function2, tonation, 4);
            UserStack stack3 = new(function3, tonation, 8);
            UserStack stack4 = new(function4, tonation, 8);

            stack1.SetSoprano(C_1_S, quarterNote);
            stack1.SetAlto(E_1_A, quarterNote);
            stack1.SetTenore(G_1_T, quarterNote);
            stack1.SetBass(C_1_B, quarterNote);

            stack2.SetSoprano(C_2_S, quarterNote);
            stack2.SetAlto(F_2_A, quarterNote);
            stack2.SetTenore(A_2_T, quarterNote);
            stack2.SetBass(C_2_B, quarterNote);

            stack3.SetSoprano(B_3_S, quarterNote);
            stack3.SetAlto(G_3_A, quarterNote);
            stack3.SetTenore(G_3_T, quarterNote);
            stack3.SetBass(D_3_B, quarterNote);

            stack4.SetSoprano(C_4_S, halfNoteDotted);
            stack4.SetAlto(E_4_A, halfNoteDotted);
            stack4.SetTenore(G_4_T, halfNoteDotted);
            stack4.SetBass(C_4_B, halfNoteDotted);

            Bar bar1 = new(tonation, meter);
            Bar bar2 = new(tonation, meter);

            bar1.AddFunction(function1);
            bar1.AddFunction(function2);
            bar1.AddFunction(function3);
            bar2.AddFunction(function4);

            UserBar userBar1 = new();
            UserBar userBar2 = new();

            userBar1.AddStack(stack1);
            userBar1.AddStack(stack2);
            userBar1.AddStack(stack3);
            userBar2.AddStack(stack4);

            Algorithm.Music.Task task = new(length: 2);
            task.AddBarsRange([bar1, bar2]);            

            var parsedJson = TaskParser.ParseTaskToJson([userBar1, userBar2], tonation, meter);
            var testString = "{\"meterValue\":4,\"meterCount\":3,\"sharpsCount\":0,\"flatsCount\":0,\"jsonNotes\":[{\"Line\":1.5,\"AccidentalName\":\"\",\"Voice\":\"SOPRANO\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":0},{\"Line\":4.0,\"AccidentalName\":\"\",\"Voice\":\"ALTO\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":0},{\"Line\":1.5,\"AccidentalName\":\"\",\"Voice\":\"TENORE\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":0},{\"Line\":3.5,\"AccidentalName\":\"\",\"Voice\":\"BASS\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":0},{\"Line\":1.5,\"AccidentalName\":\"\",\"Voice\":\"SOPRANO\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":1},{\"Line\":3.5,\"AccidentalName\":\"\",\"Voice\":\"ALTO\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":1},{\"Line\":1.0,\"AccidentalName\":\"\",\"Voice\":\"TENORE\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":1},{\"Line\":3.5,\"AccidentalName\":\"\",\"Voice\":\"BASS\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":1},{\"Line\":2.0,\"AccidentalName\":\"\",\"Voice\":\"SOPRANO\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":2},{\"Line\":4.0,\"AccidentalName\":\"\",\"Voice\":\"ALTO\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":2},{\"Line\":1.5,\"AccidentalName\":\"\",\"Voice\":\"TENORE\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":2},{\"Line\":3.5,\"AccidentalName\":\"\",\"Voice\":\"BASS\",\"Value\":4,\"BarIndex\":0,\"VerticalIndex\":2},{\"Line\":1.5,\"AccidentalName\":\"\",\"Voice\":\"SOPRANO\",\"Value\":12,\"BarIndex\":1,\"VerticalIndex\":0}]}";

            Assert.Equal(parsedJson, testString);
        }
    }
}
