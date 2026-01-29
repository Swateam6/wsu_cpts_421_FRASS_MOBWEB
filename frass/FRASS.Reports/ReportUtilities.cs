using System;
using Aspose.Pdf.Generator;
using System.Collections.Generic;

namespace FRASS.Reports
{
	internal class ReportUtilities
	{
		private int FontSize { get; set; }
		public ReportUtilities() 
		{
			FontSize = 8;
		}
		public ReportUtilities(int fontSize) 
		{
			FontSize = fontSize;
		}
		private MarginInfo _sm { get; set; }
		public MarginInfo SectionMargin
		{
			get
			{
				if (_sm == null)
				{
					_sm = new MarginInfo();
					_sm.Left = 20;
					_sm.Right = 20;
					_sm.Bottom = 20;
					_sm.Top = 20;
				}
				return _sm;
			}
		}
		private BorderInfo _rbi { get; set; }
		public BorderInfo RowBottomBorderInfo
		{
			get
			{
				if (_rbi == null)
				{
					_rbi = new BorderInfo((int)BorderSide.Bottom, .5f, new Color(128, 128, 128));
				}
				return _rbi;
			}
		}
		private BorderInfo _rbil { get; set; }
		public BorderInfo RowBottomBorderInfoLight
		{
			get
			{
				if (_rbil == null)
				{
					_rbil = new BorderInfo((int)BorderSide.Bottom, .5f, new Color(225, 225, 225));
				}
				return _rbil;
			}
		}

		public Cell AddImageCell(Row row, string filePath, AlignmentType alignment, ImageFileType fileType)
		{
			var cell = row.Cells.Add();
			var image = new Image();
			image.ImageInfo.ImageFileType = fileType;
			image.ImageInfo.Alignment = alignment;
			image.ImageInfo.File = filePath;
			cell.Paragraphs.Add(image);
			cell.Alignment = alignment;
			return cell;
		}

		public Cell AddImageCell(Row row, string filePath, AlignmentType alignment, ImageFileType fileType, float scale)
		{
			var cell = row.Cells.Add();
			var image = new Image();
			image.ImageInfo.ImageFileType = fileType;
			image.ImageInfo.File = filePath;
			image.ImageScale = scale;
			cell.Paragraphs.Add(image);
			cell.Alignment = alignment;
			return cell;
		}

		public Cell AddLabelCell(Row row, string text, AlignmentType alignment)
		{
			return NewCell(row, text, alignment, true, false);
		}
		public Cell AddLabelCell(Row row, string text, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, text, alignment, true, false);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}

		public Cell AddLabelCellNonBold(Row row, string text, AlignmentType alignment)
		{
			return NewCell(row, text, alignment, false, false);
		}
		public Cell AddLabelCellNonBold(Row row, string text, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, text, alignment, false, false);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		public Cell AddLabelCellNonBold(Row row, List<string> texts, AlignmentType alignment)
		{
			var cell = NewCell(row, texts, alignment, false, false);
			return cell;
		}
		public Cell AddLabelCellNonBold(Row row, List<string> texts, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, texts, alignment, false, false);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}

		public Cell AddLabelCellGray(Row row, string text, AlignmentType alignment)
		{
			var cell = NewCell(row, text, alignment, true, false);
			cell.BackgroundColor = Color_LiteGray;
			return cell;
		}
		public Cell AddLabelCellGray(Row row, string text, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, text, alignment, true, false);
			cell.BackgroundColor = Color_LiteGray;
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		public Cell AddHighlightCell(Row row, string text, AlignmentType alignment)
		{
			return NewCell(row, text, alignment, false, true);
		}
		public Cell AddHighlightCell(Row row, string text, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, text, alignment, false, true);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		public Cell AddHighlightCellBold(Row row, string text, AlignmentType alignment)
		{
			return NewCell(row, text, alignment, true, true);
		}
		public Cell AddHighlightCellBold(Row row, string text, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, text, alignment, true, true);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		private Cell NewCell(Row row, string text, AlignmentType alignment, bool bold, bool highlight)
		{
			var list = new List<string>();
			list.Add(text);
			return NewCell(row, list, alignment, bold, highlight);
		}


		public Cell AddLabelCell(Row row, List<string> texts, AlignmentType alignment)
		{
			return NewCell(row, texts, alignment, true, false);
		}
		public Cell AddLabelCell(Row row, List<string> texts, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, texts, alignment, true, false);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		public Cell AddLabelCellGray(Row row, List<string> texts, AlignmentType alignment) 
		{
			var cell = NewCell(row, texts, alignment, true, false);
			cell.BackgroundColor = Color_LiteGray;
			return cell;
		}
		public Cell AddLabelCellGray(Row row, List<string> texts, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, texts, alignment, true, false);
			cell.BackgroundColor = Color_LiteGray;
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		
		public Cell AddHighlightCell(Row row, List<string> texts, AlignmentType alignment)
		{
			return NewCell(row, texts, alignment, false, true);
		}
		public Cell AddHighlightCell(Row row, List<string> texts, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, texts, alignment, false, true);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		public Cell AddHighlightCellBold(Row row, List<string> texts, AlignmentType alignment)
		{
			return NewCell(row, texts, alignment, true, true);
		}
		public Cell AddHighlightCellBold(Row row, List<string> texts, AlignmentType alignment, int columnSpan)
		{
			var cell = NewCell(row, texts, alignment, true, true);
			cell.ColumnsSpan = columnSpan;
			return cell;
		}
		private Cell NewCell(Row row, List<string> texts, AlignmentType alignment, bool bold, bool highlight)
		{
			var cell = row.Cells.Add();
			foreach (var text in texts)
			{
				var txt = new Text(text);
				if (bold)
				{
					switch(alignment)
					{
						case AlignmentType.Left:
							txt.TextInfo = TextInfoBoldLeft;
							break;
						case AlignmentType.Center:
							txt.TextInfo = TextInfoBoldCenter;
							break;
						case AlignmentType.Right:
							txt.TextInfo = TextInfoBoldRight;
							break;
					}
				}
				else
				{
					switch (alignment)
					{
						case AlignmentType.Left:
							txt.TextInfo = TextInfoLeft;
							break;
						case AlignmentType.Center:
							txt.TextInfo = TextInfoCenter;
							break;
						case AlignmentType.Right:
							txt.TextInfo = TextInfoRight;
							break;
					}
				}
				if (highlight)
				{
					cell.BackgroundColor = Color_Brown;
				}
				cell.Paragraphs.Add(txt);
			}
			cell.Alignment = alignment;
			cell.Padding = CellPadding;
			return cell;
		}

		public Table GetNewTable()
		{
			var table = GetNewTable(1);
			return table;
		}
		public Table GetNewTable(int columns)
		{
			var table = new Table();
			var str = GetColumnWidths(columns);
			table.ColumnWidths = str;
			table.Border = BorderInfo;
			return table;
		}

		public Table GetNewBlankTable()
		{
			return GetNewBlankTable(1);
		}

		public Table GetNewBlankTable(int columns)
		{
			var table = new Table();
			var str = GetColumnWidths(columns);
			table.ColumnWidths = str;
			return table;
		}

		public Table GetBlankTable()
		{
			var table = new Table();
			table.ColumnWidths = "100%";
			table.Border = new BorderInfo();
			table.Rows.Add().Cells.Add(" ");
			return table;
		}
		public Table AppendNewOneColumnTable(Table table)
		{
			return AppendNewTable(table, 1);
		}
		public Table AppendNewSixColumnTable(Table table)
		{
			return AppendNewTable(table, 6);
		}
		public Table AppendNewTable(Table table, int cols)
		{
			var row = table.Rows.Add();
			var cell = row.Cells.Add();
			var newTable = GetNewVariableColumnTable(cell, cols);
			cell.Paragraphs.Add(newTable);
			row.Border = BorderInfo;
			return newTable;
		}
		public Table AppendNewTableNoBorder(Table table, int cols)
		{
			var row = table.Rows.Add();
			var cell = row.Cells.Add();
			var newTable = GetNewVariableColumnTable(cell, cols);
			cell.Paragraphs.Add(newTable);
			return newTable;
		}
		public Table GetNewVariableColumnTable(Cell cell, int cols)
		{
			var str = GetColumnWidths(cols);
			return GetNewVariableColumnTable(cell, str);
		}
		private string GetColumnWidths(int cols)
		{
			var width = 100 / cols;
			var tots = 100;
			var ict = 1;
			var str = "";
			while (ict <= cols)
			{
				if (ict == cols)
				{
					width = width + (tots - width);
					str = str + width.ToString() + "%";
				}
				else
				{
					str = str + width.ToString() + "%" + " ";
				}
				tots = tots - width;
				ict = ict + 1;

			}
			return str;
		}

		public Table GetNewVariableColumnTable(Cell cell, string widths)
		{
			var newTable = new Table(cell);
			newTable.ColumnWidths = widths;
			newTable.BackgroundColor = Color_White;
			return newTable;
		}

		private BorderInfo _bi { get; set; }
		private BorderInfo BorderInfo
		{
			get
			{
				if (_bi == null)
				{
					_bi = new BorderInfo((int)BorderSide.All, .5f, new Color(128, 128, 128));
				}
				return _bi;
			}
		}

		private MarginInfo _cp { get; set; }
		private MarginInfo CellPadding
		{
			get
			{
				if (_cp == null)
				{
					_cp = new MarginInfo();
					_cp.Top = 5f;
					_cp.Left = 5f;
					_cp.Right = 5f;
					_cp.Bottom = 5f;
				}
				return _cp;
			}

		}

		private TextInfo _lti { get; set; }
		public TextInfo LinkTextInfo
		{
			get
			{
				if (_lti == null)
				{
					_lti = new TextInfo();
					_lti.FontName = "Arial";
					_lti.FontSize = FontSize;
					_lti.Color = new Color(0, 0, 255);
					_lti.IsUnderline = true;
					_lti.Alignment = AlignmentType.Left;
				}
				return _lti;
			}
		}

		private TextInfo _til { get; set; }
		private TextInfo TextInfoLeft
		{
			get
			{
				if (_til == null)
				{
					_til = new TextInfo();
					_til.FontName = "Arial";
					_til.FontSize = FontSize;
					_til.Color = new Color(0, 0, 0);
					_til.Alignment = AlignmentType.Left;
				}
				return _til;
			}
		}
		private TextInfo _tic { get; set; }
		public TextInfo TextInfoCenter
		{
			get
			{
				if (_tic == null)
				{
					_tic = new TextInfo();
					_tic.FontName = "Arial";
					_tic.FontSize = FontSize;
					_tic.Color = new Color(0, 0, 0);
					_tic.Alignment = AlignmentType.Center;
				}
				return _tic;
			}
		}
		private TextInfo _tir { get; set; }
		public TextInfo TextInfoRight
		{
			get
			{
				if (_tir == null)
				{
					_tir = new TextInfo();
					_tir.FontName = "Arial";
					_tir.FontSize = FontSize;
					_tir.Color = new Color(0, 0, 0);
					_tir.Alignment = AlignmentType.Right;
				}
				return _tir;
			}
		}

		private TextInfo _tibl { get; set; }
		private TextInfo TextInfoBoldLeft
		{
			get
			{
				if (_tibl == null)
				{
					_tibl = new TextInfo();
					_tibl.FontName = "Arial";
					_tibl.FontSize = FontSize;
					_tibl.Color = new Color(0, 0, 0);
					_tibl.Alignment = AlignmentType.Left;
					_tibl.IsTrueTypeFontBold = true;
				}
				return _tibl;
			}
		}
		private TextInfo _tibc { get; set; }
		private TextInfo TextInfoBoldCenter
		{
			get
			{
				if (_tibc == null)
				{
					_tibc = new TextInfo();
					_tibc.FontName = "Arial";
					_tibc.FontSize = FontSize;
					_tibc.Color = new Color(0, 0, 0);
					_tibc.Alignment = AlignmentType.Center;
					_tibc.IsTrueTypeFontBold = true;
				}
				return _tibc;
			}
		}
		private TextInfo _tibr { get; set; }
		private TextInfo TextInfoBoldRight
		{
			get
			{
				if (_tibr == null)
				{
					_tibr = new TextInfo();
					_tibr.FontName = "Arial";
					_tibr.FontSize = FontSize;
					_tibr.Color = new Color(0, 0, 0);
					_tibr.Alignment = AlignmentType.Right;
					_tibr.IsTrueTypeFontBold = true;
				}
				return _tibr;
			}
		}
		
		private Color _cb { get; set; }
		public Color Color_Brown
		{
			get
			{
				if (_cb == null)
				{
					_cb = new Color(221, 217, 195);
				}
				return _cb;
			}
		}
		private Color _cg { get; set; }
		public Color Color_Gray
		{
			get
			{
				if (_cg == null)
				{
					_cg = new Color(195, 195, 195);
				}
				return _cg;
			}
		}
		private Color _clg { get; set; }
		public Color Color_LiteGray
		{
			get
			{
				if (_clg == null)
				{
					_clg = new Color(225, 225, 225);
				}
				return _clg;
			}
		}
		private Color _cw { get; set; }
		public Color Color_White
		{
			get
			{
				if (_cw == null)
				{
					_cw = new Color(255, 255, 255);
				}
				return _cw;
			}
		}
	}
}