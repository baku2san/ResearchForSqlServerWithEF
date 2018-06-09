using ResearchForSqlServerWithEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity.SqlServer;

namespace ResearchForSqlServerWithEF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var entity = new testdbEntities();
            var list = entity.TableB.Select(s =>
                new
                {
                    DateShorten = s.DateTime.Year + "/" + s.DateTime.Month + "/" + s.DateTime.Day,
                    DateShortenNullable = (s.DateTimeNullable.HasValue) ? s.DateTimeNullable.Value.Year + "/" + s.DateTimeNullable.Value.Month + "/" + s.DateTimeNullable.Value.Day: string.Empty
                });
            list.ToList().ForEach(f=> System.Diagnostics.Debug.Print(f.DateShorten + " " + f.DateShortenNullable));

            #region DateTime expression
            // OK : DateName使うぐらいなら、単純にこっちかな
            //var list = entity.TableB.Select(s =>
            //new
            //{
            //    DateShorten = s.DateTime.Year + "/" + s.DateTime.Month + "/" + s.DateTime.Day,
            //    DateShortenNullable = (s.DateTimeNullable.HasValue) ? s.DateTimeNullable.Value.Year + "/" + s.DateTimeNullable.Value.Month + "/" + s.DateTimeNullable.Value.Day : string.Empty
            //});

            // OK : 面倒だけど、これかなぁ
            //var list = entity.TableB.Select(s =>
            //    new
            //    {
            //        DateShorten = SqlFunctions.DateName("yyyy", s.DateTime) + "/" + SqlFunctions.DateName("MM", s.DateTime) + "/" + SqlFunctions.DateName("dd", s.DateTime),
            //        DateShortenNullable = (s.DateTimeNullable.HasValue) ? SqlFunctions.DateName("yyyy", s.DateTimeNullable) + "/" + SqlFunctions.DateName("MM", s.DateTimeNullable) + "/" + SqlFunctions.DateName("dd", s.DateTimeNullable) : string.Empty
            //    });

            // OK: TruncateTime()   結局時刻がResetされているだけ
            //2018 / 06 / 08 0:00:00 6
            //2018 / 06 / 08 0:00:00
            //var list = entity.TableB.Select(s => new { DateShorten = System.Data.Entity.DbFunctions.TruncateTime(s.DateTime), DateShortenNullable = System.Data.Entity.SqlServer.SqlFunctions.DateName("day", s.DateTimeNullable) });

            // OK SqlFunctions も、EF 用ならOK
            //var list = entity.TableB.Select(s => new { DateShorten = System.Data.Entity.SqlServer.SqlFunctions.DateName("day", s.DateTime), DateShortenNullable = System.Data.Entity.SqlServer.SqlFunctions.DateName("day", s.DateTimeNullable) });

            // OK : 単純な ToString() : Null も表示されない。ただ、書式がね
            // ★ ToString("yyyy") とか、ToShortDateString() とかはNG
            //06  8 2018 10:52PM 06  6 2018 12:00AM
            //06  8 2018 10:52PM
            //var list = entity.TableB.Select(s => new { DateShorten = s.DateTime.ToString(), DateShortenNullable = s.DateTimeNullable.ToString() });

            // OK DateTime そのままなら当然両方OK
            // OK            var list = entity.TableB.Select(s => new { s.DateTime, s.DateTimeNullable });
            // OK           list.ToList().ForEach(f=> System.Diagnostics.Debug.Print(f.DateTime.ToShortDateString() + " " + f.DateTimeNullable.ToString()));


            // NG　Linq to Entitiesでは駄目って
            //var list = entity.TableB.Select(s => new { DateShorten = string.Format("{0:yyyy/MM/dd}", s.DateTime), DateShortenNullable = System.Data.Entity.SqlServer.SqlFunctions.DateName("day", s.DateTimeNullable) });

            // NG　Linq to Entitiesでは駄目って
            //var list = entity.TableB.Select(s => new { DateShorten = System.Data.Objects.SqlClient.SqlFunctions.DateName("day", s.DateTime), DateShortenNullable = s.DateTimeNullable.ToString() });

            // NG　Linq to Entitiesでは駄目って
            //var list = entity.TableB.Select(s => new { DateShorten = s.DateTime.ToShortDateString(), DateShortenNullable = s.DateTimeNullable.ToString() });

            #endregion

        }
    }
}
