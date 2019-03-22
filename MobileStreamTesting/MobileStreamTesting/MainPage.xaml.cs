using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CSJ2K.j2k.util;
using System.Drawing;
using FFImageLoading;
using FFImageLoading.Forms;

namespace MobileStreamTesting
{
    public partial class MainPage : ContentPage
    {
        int index = 1;
        public CachedImage originalImage;

        public MainPage()
        {
            InitializeComponent();
            StartUp();
        }

        public void StartUp()
        {
            originalImage = new CachedImage();
            PreviewImage.Source = ImageSource.FromStream(()=>App.assetOpener.GetFrame(index));
            originalImage.Source = ImageSource.FromStream(() => App.assetOpener.GetFrame(index));
        }

        public void GetNextFrame(object sender, EventArgs e)
        {
            index++;
            index = Math.Min(16, index);
            PreviewImage.Source = ImageSource.FromStream(() => App.assetOpener.GetFrame(index));

            if (index == 16)
            {
                index = 0;
            }
            //await Task.Delay(25);
            //BackgroundImage.Source = PreviewImage.Source;
        }

        public void GetPrevFrame(object sender, EventArgs e)
        {
            index--;
            index = Math.Max(1, index);
            PreviewImage.Source = ImageSource.FromStream(() => App.assetOpener.GetFrame(index));
        }

        public async void JpegResolutionChanged(object sender, EventArgs e)
        {
            double resQuality;
            if (!double.TryParse(ResolutionEntry.Text, out resQuality))
            {
                resQuality = 50;
            }
            if (resQuality > 100)
            {
                resQuality = 100;
            }
            else if (resQuality < 1)
            {
                resQuality = 1;
            }
            int quality;
            if (!int.TryParse(QualityEntry.Text, out quality))
            {
                quality = 50;
            }
            if (quality > 100)
            {
                quality = 100;
            }
            else if (quality < 1)
            {
                quality = 1;
            }
            Stream stream = new MemoryStream(await ImageCorrection.ImageResizer.ResizeImageToQuality(PreviewImage, quality, resQuality/100.0));
            PreviewImage.Source = ImageSource.FromStream( () => stream);
        }

        public async void JpegQualityChanged(object sender, EventArgs e)
        {
            int quality;
            if (!int.TryParse(QualityEntry.Text, out quality))
            {
                quality = 50;
            }
            if (quality > 100)
            {
                quality = 100;
            }
            else if (quality < 1)
            {
                quality = 1;
            }
            double resQuality;
            if (!double.TryParse(ResolutionEntry.Text, out resQuality))
            {
                resQuality = 50;
            }
            if (resQuality > 100)
            {
                resQuality = 100;
            }
            else if (resQuality < 1)
            {
                resQuality = 1;
            }
            Stream stream = new MemoryStream(await ImageCorrection.ImageResizer.ResizeImageToQuality(PreviewImage, quality, resQuality/100.0));
            PreviewImage.Source = ImageSource.FromStream(() => stream);
        }

        public void ResetImage(object sender ,EventArgs e)
        {
            PreviewImage.Source = ImageSource.FromStream(() => App.assetOpener.GetFrame(index));
        }

        private static String[][] encoder_pinfo =
           {
                new string[]
                    {
                        "debug", null,
                        "Print debugging messages when an error is encountered.",
                        "off"
                    },
                new string[]
                    {
                        "disable_jp2_extension", "[on|off]",
                        "JJ2000 automatically adds .jp2 extension when using 'file_format'"
                        + "option. This option disables it when on.",
                        "off"
                    },
                new string[]
                    {
                        "file_format", "[on|off]",
                        "Puts the JPEG 2000 codestream in a JP2 file format wrapper.",
                        "on"
                    },
                new string[]
                    {
                        "pph_tile", "[on|off]",
                        "Packs the packet headers in the tile headers.", "off"
                    },
                new string[]
                    {
                        "pph_main", "[on|off]",
                        "Packs the packet headers in the main header.", "off"
                    },
                new string[]
                    {
                        "pfile", "<filename of arguments file>",
                        "Loads the arguments from the specified file. Arguments that are "
                        + "specified on the command line override the ones from the file.\n"
                        + "The arguments file is a simple text file with one argument per "
                        + "line of the following form:\n"
                        + "  <argument name>=<argument value>\n"
                        + "If the argument is of boolean type (i.e. its presence turns a "
                        + "feature on), then the 'on' value turns it on, while the 'off' "
                        + "value turns it off. The argument name does not include the '-' "
                        + "or '+' character. Long lines can be broken into several lines "
                        + "by terminating them with '\'. Lines starting with '#' are "
                        + "considered as comments. This option is not recursive: any 'pfile' "
                        + "argument appearing in the file is ignored.",
                        null
                    },
                new string[]
                    {
                        "tile_parts", "<packets per tile-part>",
                        "This option specifies the maximum number of packets to have in "
                        + "one tile-part. 0 means include all packets in first tile-part "
                        + "of each tile",
                        "0"
                    },
                new string[]
                    {
                        "tiles", "<nominal tile width> <nominal tile height>",
                        "This option specifies the maximum tile dimensions to use. "
                        + "If both dimensions are 0 then no tiling is used.",
                        "0 0"
                    },
                new string[]
                    {
                        "ref", "<x> <y>",
                        "Sets the origin of the image in the canvas system. It sets the "
                        + "coordinate of the top-left corner of the image reference grid, "
                        + "with respect to the canvas origin",
                        "0 0"
                    },
                new string[]
                    {
                        "tref", "<x> <y>",
                        "Sets the origin of the tile partitioning on the reference grid, "
                        + "with respect to the canvas origin. The value of 'x' ('y') "
                        + "specified can not be larger than the 'x' one specified in the ref "
                        + "option.",
                        "0 0"
                    },
                new string[]
                    {
                        "rate", "<output bitrate in bpp>",
                        "This is the output bitrate of the codestream in bits per pixel."
                        + " When equal to -1, no image information (beside quantization "
                        + "effects) is discarded during compression.\n"
                        + "Note: In the case where '-file_format' option is used, the "
                        + "resulting file may have a larger bitrate.",
                        "-1"
                    },
                new string[]
                    {
                        "lossless", "[on|off]",
                        "Specifies a lossless compression for the encoder. This options"
                        + " is equivalent to use reversible quantization ('-Qtype "
                        + "reversible')"
                        + " and 5x3 wavelet filters pair ('-Ffilters w5x3'). Note that "
                        + "this option cannot be used with '-rate'. When this option is "
                        + "off, the quantization type and the filters pair is defined by "
                        + "'-Qtype' and '-Ffilters' respectively.",
                        "off"
                    },
                new string[]
                    {
                        "i", "<image file> [,<image file> [,<image file> ... ]]",
                        "Mandatory argument. This option specifies the name of the input "
                        + "image files. If several image files are provided, they have to be"
                        + " separated by commas in the command line. Supported formats are "
                        + "PGM (raw), PPM (raw) and PGX, "
                        + "which is a simple extension of the PGM file format for single "
                        + "component data supporting arbitrary bitdepths. If the extension "
                        + "is '.pgm', PGM-raw file format is assumed, if the extension is "
                        + "'.ppm', PPM-raw file format is assumed, otherwise PGX file "
                        + "format is assumed. PGM and PPM files are assumed to be 8 bits "
                        + "deep. A multi-component image can be specified by either "
                        + "specifying several PPM and/or PGX files, or by specifying one "
                        + "PPM file.",
                        null
                    },
                new string[]
                    {
                        "o", "<file name>",
                        "Mandatory argument. This option specifies the name of the output "
                        + "file to which the codestream will be written.",
                        null
                    },
                new string[]
                    {
                        "verbose", null,
                        "Prints information about the obtained bit stream.", "on"
                    },
                new string[]
                    {
                        "v", "[on|off]", "Prints version and copyright information.",
                        "off"
                    },
                new string[]
                    {
                        "u", "[on|off]",
                        "Prints usage information. "
                        + "If specified all other arguments (except 'v') are ignored",
                        "off"
                    },
};
    }
}
