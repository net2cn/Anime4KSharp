# Anime4KSharp
A C# implementation of Anime4K that execute compelete on CPU for getting a better understanding of the algorithm.

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

![Original image](images/f155.png?raw=true)

Step 1 uses bicubic interpolation to upscale image.

![Upscaled image](images/f155_Bicubic.png?raw=true)

Step 2 calculates luminances for all pixel and store them in the unused alpha channel.

![Luminance map in the alpha channel](images/f155_Luminance.png?raw=true)

Step 3 pushes color based on luminance.
![Pushed Color](images/f155_PushColor.png?raw=true)

Step 4 detects edges of the luminance map with Sobel filter. Still, we store them in the alpha channel. However, you will have to invert it before you store it in the alpha channel.

![Gradient map](images/f155_Grad.png?raw=true)
![Inverted gradient map in the alpha channel](images/f155_InvertedGrad.png?raw=true)

Step 5 is nearly the same as step 3. Instead of getting the lightest color, we use the average color here. Of course, you will need to fill each pixel's alpha with 255 to dump the alpha channel since we don't want viewer to have a weird image with all the edges seem to be transparent.

![Final result](images/f155_Upscaled.png?raw=true)

# Acknowledgements
This repository contains the following image materials:

- 【物語×放置ゲームコレクション】ゴスロリ吸血鬼 from Niconi Commons

  https://commons.nicovideo.jp/material/nc160359

This repository is created only for learning purpose. I DO NOT take any responsibilities for any possible damages.

2019, net2cn.