# Anime4KSharp
A C# implementation of [Anime4K](https://github.com/bloc97/Anime4K) that execute compelete on CPU for getting a better understanding of the algorithm. It works more like a filter than a upscaler in my opinion.

# Usage
dotnet Anime4KSharp.dll [input_path] [output_path] [(Optional)scale] [(Optional)push_strength] [(Optional)push_gradient_strength]

default optional parameters:
```
scale = 2f;
pushGradStrength = scale / 2f;
pushStrength = scale / 6f;
```

# Write-up
Actually this algorithm is as dead simple as bloc97 described in this [pseudo-preprint](https://github.com/bloc97/Anime4K/blob/master/Preprint.md).

Basically this can be described in 5 steps.

Assuming we have the following image:

![Original image](images/Rand0mZ_King_Downscaled_256px.png?raw=true)

Step 1 uses bicubic interpolation to upscale image.

![Upscaled image](images/Rand0mZ_King_Bicubic.png?raw=true)

Step 2 calculates luminances for all pixel and store them in the unused alpha channel.

![Luminance map in the alpha channel](images/Rand0mZ_King_Luminance.png?raw=true)

Step 3 pushes color based on luminance. Well after doing this we will get an image that looks nearly the same? No, there are differences. Look carefully at the edge of different color and lines, you will notice that they are lighter than the original bicubic upscaled one, especially the lines.

![Pushed Color](images/Rand0mZ_King_PushColor.png?raw=true)

Step 4 detects edges of the luminance map with Sobel filter. Still, we store them in the alpha channel. However, you will have to invert it before you store it in the alpha channel.

![Gradient map](images/Rand0mZ_King_Gradient.png?raw=true)
![Inverted gradient map in the alpha channel](images/Rand0mZ_King_InvertedGradient.png?raw=true)

Step 5 is nearly the same as step 3. Instead of getting the lightest color, we use the average color here (doing this can remove some unwanted noise caused by bicubic). And yes, you will want to fill each pixel's alpha with 255 to dump the alpha channel (which contains our gradient map) since we don't want viewer to have a weird image with all the edges seem to be transparent. And here's our final result.

![Final result](images/Rand0mZ_King_FinalResult.png?raw=true)

FYI, here is a zoomed-in comparison between bicubic interpolation and 2 pass Anime4K.

![Comparison](images/Comparison.png?raw=true)

# TODO
- Apply FXAA filter to lines in order to get sharper (and smoother) edges.

# Acknowledgements
This repository contains images by [Rand0mZ](https://github.com/Rand0mZharp) and are authorized to used. THEY ARE FOR DEMOSTRATION ONLY, PLEASE DO NOT USE WITHOUT PERMISSION!

This repository is created only for learning purpose. I DO NOT take any responsibilities for any possible damages.

2019, net2cn, in assistance with Rand0mZ.