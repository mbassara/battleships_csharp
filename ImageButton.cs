using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Battleships
{
    class ImageButton : PictureBox
    {
        public enum SCALE { SCALE_SMALL, SCALE_MID, SCALE_BIG };

        private SCALE scale;
        public new SCALE Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                switch (scale)
                {
                    case SCALE.SCALE_SMALL:
                        this.Size = new Size(67, 35);
                        this.Image = global::Battleships.Properties.Resources.ic_button_small;
                        break;
                    case SCALE.SCALE_MID:
                        this.Size = new Size(100, 35);
                        this.Image = global::Battleships.Properties.Resources.ic_button_mid;
                        break;
                    default:
                        scale = SCALE.SCALE_BIG;
                        this.Size = new Size(133, 35);
                        this.Image = global::Battleships.Properties.Resources.ic_button_large;
                        break;
                }
            }
        }

        private String text = "";
        new public String Text
        {
            get { return text; }
            set
            {
                text = value;
                Refresh();
            }
        }

        new public bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;

                if (value)
                {
                    Paint -= onPaintGrey;
                    Paint += onPaintWhite;
                }
                else
                {
                    Paint -= onPaintWhite;
                    Paint += onPaintGrey;
                }
                Refresh();
            }
        }

        public ImageButton(String text) : this(SCALE.SCALE_BIG, text)
        {
        }

        public ImageButton(SCALE scale, String text)
        {
            this.Scale = scale;
            this.BackColor = Color.Transparent;
            this.text = text;
            this.Paint += onPaintWhite;
            this.MouseDown += onClick;
            this.MouseUp += onRelase;
        }

        private void onClick(object sender, EventArgs e)
        {
            switch (scale)
            {
                case SCALE.SCALE_SMALL:
                    this.Image = global::Battleships.Properties.Resources.ic_button_small_clicked;
                    break;
                case SCALE.SCALE_MID:
                    this.Image = global::Battleships.Properties.Resources.ic_button_mid_clicked;
                    break;
                case SCALE.SCALE_BIG:
                    this.Image = global::Battleships.Properties.Resources.ic_button_large_clicked;
                    break;
                default:
                    break;
            }

        }

        private void onRelase(object sender, EventArgs e)
        {
            switch (scale)
            {
                case SCALE.SCALE_SMALL:
                    this.Image = global::Battleships.Properties.Resources.ic_button_small;
                    break;
                case SCALE.SCALE_MID:
                    this.Image = global::Battleships.Properties.Resources.ic_button_mid;
                    break;
                case SCALE.SCALE_BIG:
                    this.Image = global::Battleships.Properties.Resources.ic_button_large;
                    break;
                default:
                    break;
            }

        }

        private void onPaintWhite(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using (Font myFont = new Font("Arial", 12))
            {
                SizeF textSize = e.Graphics.MeasureString(text, myFont);
                PointF location = new PointF();
                location.X = this.Size.Width / 2 - textSize.Width / 2;
                location.Y = this.Size.Height / 2 - textSize.Height / 2;
                e.Graphics.DrawString(text , myFont, Brushes.White, location);
            }
        }

        private void onPaintGrey(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using (Font myFont = new Font("Arial", 12))
            {
                SizeF textSize = e.Graphics.MeasureString(text, myFont);
                PointF location = new PointF();
                location.X = this.Size.Width / 2 - textSize.Width / 2;
                location.Y = this.Size.Height / 2 - textSize.Height / 2;
                e.Graphics.DrawString(text, myFont, Brushes.DarkGray, location);
            }
        }
    }
}
